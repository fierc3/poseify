import { EstimationList } from "../../components/estimation-list/estimation-list";
import { Box, LinearProgress, Paper, Stack, Typography } from "@mui/material";

const Dashboard = () => {
  return (
    <Stack sx={{ width: "100vw", height: "90vh" }} direction="column" gap={1} flexWrap="wrap">
      <Stack  flexGrow={1} spacing={{ xs: 1, sm: 2 }} direction="row" useFlexGap flexWrap="wrap">
        <Box flexGrow={7} sx={{ }}>
          <Typography variant="h5">
            Upload Place Holder
          </Typography>
        </Box>
        <Paper sx={{ display: { xs: 'none', sm: 'block' } }} style={{ backgroundColor: "black", flexGrow: 3, marginTop: -3, alignContent: "center" }}>
          <Stack flexGrow={1} spacing={{ xs: 3, sm: 4 }} direction="column" useFlexGap flexWrap="wrap" paddingTop={2}>
            <Box>
              <Typography color={"white"} alignContent={"center"} variant="subtitle1">
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
      <Box flexGrow={6} sx={{ width: "100vw" }}>
        <EstimationList />
      </Box>
    </Stack>
  );
};

export default Dashboard;
