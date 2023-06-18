import { EstimationList } from "../../components/estimation-list/estimation-list";
import { Box, LinearProgress, Paper, Stack, Typography } from "@mui/material";
import useClaims from "../../helpers/claims";
import { useCallback, useMemo } from "react";
import { UploadButton } from "../../components/upload-button/upload-button";
import useEstimations from "../../helpers/estimations";
import { EstimationState } from "../../helpers/api.types";

const Dashboard = () => {

  const { data: claimsData } = useClaims()
  const { data: rawEstimations, isFetched } = useEstimations();

  const getDisplayName = useCallback(() => {
    var displayName = (claimsData as { type: string; value: string }[])?.filter(x => x.type === "name")[0]?.value
    if (displayName) {
      return ', ' + displayName;
    }
    return ''
  }, [claimsData])


  const maxQueue = 1;

  const queuedEstimations: number = useMemo(() => rawEstimations?.filter(x => x.state === EstimationState.Processing || x.state === EstimationState.Queued).length ?? 0, [rawEstimations])


  return (
    <Stack sx={{ height: "90vh" }} direction="column" gap={4} flexWrap="nowrap">
      <Stack maxWidth="xl" width={"100%"} marginTop={2} alignSelf={"center"} flexGrow={1} spacing={{ xs: 1, sm: 2 }} direction="row" useFlexGap flexWrap="wrap">
        <Box flexGrow={7} sx={{}}>
          <Typography variant="h5" textAlign={"start"} paddingLeft={3} paddingBottom={"5%"}>
            Welcome Back{getDisplayName()}
          </Typography>
          <UploadButton blocked={queuedEstimations >= maxQueue}/>
        </Box>
        <Paper sx={{ display: { xs: 'none', sm: 'block' } }} style={{ backgroundColor: "black", flexGrow: 3, alignContent: "center" }}>
          <Stack flexGrow={1} spacing={{ xs: 3, sm: 4 }} direction="column" useFlexGap flexWrap="wrap" paddingTop={2}>
            <Box>
              <Typography color={"white"} variant="subtitle1">
                My Queued Estimations
              </Typography>
              <LinearProgress color="secondary" variant="determinate" value={queuedEstimations === maxQueue ? 100 : maxQueue/100 * queuedEstimations} sx={{ width: "50%", marginLeft: "25%" }} />
              <Typography color={"white"} alignContent={"center"} variant="subtitle2">
              {isFetched ? (<>{queuedEstimations} out of 1
               </>): (<>Checking queued estimations</>)}
              </Typography>
             
            </Box>
            <Box>
              <Typography color={"white"} alignContent={"center"} variant="h5" >
                Thank you for testing Poseify Beta!
              </Typography>
              <Typography color={"white"} alignContent={"center"} variant="subtitle1" >
                <a href="https://forms.gle/29S7T5Vcz3W9kbJKA">Feedback</a> is appreciated.
              </Typography>
              <Typography color={"white"} alignContent={"center"} variant="subtitle2" >
                Estimated times for processing: <br/>
                - 50mb = 10min <br/>
                - 20mb = 2min
              </Typography>
              <Typography color={"yellow"} alignContent={"center"} variant="subtitle2" >
                <b>Reported Issues: </b> <br/> IF GRID LOADS FOREVER PLEASE RELOGIN <br/> Currently only support for .mp4 files
              </Typography>
            </Box>
          </Stack>
        </Paper>
      </Stack>
      <Box maxHeight={"70%"} maxWidth="xl" width={"100%"} alignSelf={"center"} flexGrow={2} sx={{ width: "100vw" }}>
        <EstimationList />
      </Box>
    </Stack>
  );
};

export default Dashboard;