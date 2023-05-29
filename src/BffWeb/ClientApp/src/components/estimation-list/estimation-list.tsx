import { FC, useEffect, useState } from "react";
import useEstimations from "../../helpers/estimations";
import { EstimationState, IEstimation } from "../../helpers/api.types";
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import moment from 'moment-timezone';
import ProcessingIcon from '@mui/icons-material/DirectionsRun';
import SuccessIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';
import QueueIcon from '@mui/icons-material/Queue';
import { Button, Tooltip, Typography } from "@mui/material";
import { EstimationView } from "../estimation-view/estimation-view";
import axios from "axios";

export const EstimationList: FC = () => {
  moment.tz.setDefault("Europe/Zurich");
  const { data: rawEstimations, isFetched, refetch } = useEstimations();
  const [estimations, setEstimations] = useState<IEstimation[]>([]);
  const [isDataLoaded, setDataLoad] = useState<boolean>(false);
  const [selectedEstimation, setSelectedEstimation] = useState<IEstimation | null>(null);
  /*          
           }) */
  const startColumns: GridColDef[] = [
    {
      field: "displayName",
      headerName: "Estimation",
      flex: 8
    },
    {
      headerName: "State",
      field: "state",
      flex: 5,
      renderCell: (params) => {
        return (<>
          {params.value === EstimationState.Processing && (<Tooltip title="Processing on the GPU"><Typography variant="body2"><ProcessingIcon color="action" /> Processing</Typography></Tooltip>)}
          {params.value === EstimationState.Queued && (<Tooltip title="Queued, waiting for gpu to become available"><Typography variant="body2"><QueueIcon color="action" /> Queued</Typography></Tooltip>)}
          {params.value === EstimationState.Failed && (<Tooltip title={"Something went wrong: "+ estimations.find(x => x.internalGuid === params.row.internalGuid)?.stateText ?? 'unknown'}><Typography variant="body2"><ErrorIcon color="error" />Failed</Typography></Tooltip>)}
          {params.value === EstimationState.Success && (<Typography variant="body2"><SuccessIcon color="success" />Ok</Typography>)}
        </>)
      }
    },
    {
      headerName: "Tags",
      field: "tags",
      flex: 4
    },
    {
      headerName: "Last Modifed",
      field: "modifiedDate",
      align: "right",
      minWidth: 150,
      flex: 3,
      valueFormatter: (params) => moment(params.value).fromNow()
    },
    {
      headerName: "",
      field: "actions",
      align: "right",
      minWidth: 150,
      flex: 3,
      renderCell: (params) => {
        return (<>
          <Button
            onClick={(e) => setSelectedEstimation(params.row)}
            variant="contained"
            disabled={params.row.state !== EstimationState.Success}
          >
            View
          </Button>
          <Button
            sx={{ marginLeft: 1 }}
            onClick={(e) => axios.delete(`/api/DeleteEstimation?estimationId=${params.row.internalGuid}`, { headers: { 'X-CSRF': '1' } }).then(res => {
              if (res.status === 200) {
                refetch();
              }
            })}
            variant="contained"
          >
            Delete
          </Button>
        </>)
      }
    }
  ];

  useEffect(() => {
    setEstimations(rawEstimations ?? []);
    setDataLoad(isFetched);
    //only interested in estimate reloads
    // eslint-disable-next-line react-hooks/exhaustive-deps 
  }, [rawEstimations]);

  return (
    <>
      <EstimationView estimation={selectedEstimation} open={selectedEstimation != null} handleClose={() => setSelectedEstimation(null)} />
      <DataGrid columnVisibilityModel={{ tags: window.innerWidth > 500, modifiedDate: window.innerWidth > 500 }} loading={!isDataLoaded} rows={estimations} getRowId={(row: IEstimation) => row.internalGuid} columns={startColumns} />
    </>
  );
};
