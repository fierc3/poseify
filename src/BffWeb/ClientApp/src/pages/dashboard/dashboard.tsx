import { EstimationList } from "../../components/estimation-list/estimation-list";
import { Box, LinearProgress, Paper, Stack, Typography } from "@mui/material";
import useClaims from "../../helpers/claims";
import { useCallback } from "react";

const Dashboard = () => {

  const { data } = useClaims()

  const getDisplayName = useCallback(() => {
    var displayName = (data as { type: string; value: string }[])?.filter(x => x.type === "given_name")[0]?.value
    if (displayName) {
      return ', ' + displayName;
    }
    return ''
  }, [data])

  return (
    <Stack sx={{ height: "90vh" }} direction="column" gap={4} flexWrap="nowrap">
      <Stack maxWidth="xl" width={"100%"} marginTop={2} alignSelf={"center"} flexGrow={1} spacing={{ xs: 1, sm: 2 }} direction="row" useFlexGap flexWrap="wrap">
        <Box flexGrow={7} sx={{}}>
          <Typography variant="h5" textAlign={"start"} paddingLeft={3}>
            Welcome Back{getDisplayName()}
          </Typography>
        </Box>
        <Paper sx={{ display: { xs: 'none', sm: 'block' } }} style={{ backgroundColor: "black", flexGrow: 3, alignContent: "center" }}>
          <Stack flexGrow={1} spacing={{ xs: 3, sm: 4 }} direction="column" useFlexGap flexWrap="wrap" paddingTop={2}>
            <Box>
              <Typography color={"white"} variant="subtitle1">
                Daily Limit
              </Typography>
              <LinearProgress color="secondary" variant="determinate" value={20} sx={{ width: "50%", marginLeft: "25%" }} />
              <Typography color={"white"} alignContent={"center"} variant="subtitle2">
                Used x out of y
              </Typography>
            </Box>
            <Box>
              <Typography color={"white"} alignContent={"center"} variant="h5" >
                Need more Uploads?
              </Typography>
              <Typography color={"white"} alignContent={"center"} variant="subtitle1" >
                <a href="https://google.com">Contact</a> us to request more uploads
              </Typography>
            </Box>
          </Stack>
        </Paper>
      </Stack>
      <Box height="xl" maxWidth="xl" width={"100%"} alignSelf={"center"} flexGrow={2} sx={{ width: "100vw" }}>
        <EstimationList />
      </Box>
    </Stack>
  );
};

export default Dashboard;