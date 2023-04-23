import { FC, useEffect, useState } from "react";
import useEstimations from "../../helpers/estimations";
import { EstimationState, IEstimation } from "../../helpers/api.types";
import { DataGrid, GridColDef } from '@mui/x-data-grid';
import moment from "moment";
import ProcessingIcon from '@mui/icons-material/DirectionsRun';
import SuccessIcon from '@mui/icons-material/CheckCircle';
import ErrorIcon from '@mui/icons-material/Error';
import { Button } from "@mui/material";

export const EstimationList: FC = () => {
  const { data: rawEstimations, isFetched } = useEstimations();
  const [estimations, setEstimations] = useState<IEstimation[]>([]);
  const [isDataLoaded, setDataLoad] = useState<boolean>(false);
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
      flex: 3,
      renderCell: (params) => {
        return (<>
          {params.value === EstimationState.Processing && (<ProcessingIcon color="action" />)}
          {params.value === EstimationState.Failed && (<ErrorIcon color="error" />)}
          {params.value === EstimationState.Success && (<SuccessIcon color="success" />)}
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
            onClick={(e) => console.log("tbi")}
            variant="contained"
          >
            View
          </Button>
          <Button
            onClick={(e) => console.log("tbi")}
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
      <DataGrid columnVisibilityModel={{tags:window.innerWidth > 500, modifiedDate: window.innerWidth > 500}} loading={!isDataLoaded} rows={estimations} getRowId={(row: IEstimation) => row.internalGuid} columns={startColumns} />
    </>
  );
};
