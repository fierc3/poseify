import { FC } from "react";
import { Divider, Grid, Typography } from "@mui/material";

const Home: FC = () => {
  return (
    <>
      <Grid sx={{ flexGrow: 1 }} alignItems="center"
        justifyContent="center" container spacing={3} paddingTop={4}>
        <Grid item xs={6}>
          <Typography variant="h2" sx={{display: { xs: 'none', md: 'block' }}} gutterBottom>
            Welcome to POSEIFY
          </Typography>
          <Typography variant="body1">

            <b>Welcome to our video pose estimation service!</b> We are excited to
            introduce you to our innovative solution that makes it easier than
            ever to analyze and understand human movement.
          </Typography>
          <Typography variant="body1" paddingTop={5}>
            Our service is designed to help professionals in a wide range of
            industries, from sports and fitness to healthcare and
            entertainment. Whether you're a coach looking to improve your
            athletes' form, a doctor monitoring a patient's physical therapy
            progress, or a filmmaker creating stunning action scenes, our
            technology can help you achieve your goals.
          </Typography>
        </Grid>
        <Grid item xs={12}>
        <Divider variant="middle" />
          <Typography variant="h4" paddingTop={3} paddingBottom={3}>
            POSEIFY is powered by
          </Typography>
          <Grid container justifyContent="center" spacing={2}>
            <Grid item xs={3}>
              <Typography variant="h6" sx={{display: { xs: 'none', md: 'block' }}}>
                VideoPose3d
              </Typography>
              <Typography variant="h6" sx={{display: { xs: 'block', md: 'none' }}}>
                VP3D
              </Typography>
              <Typography variant="subtitle2">
                VideoPose3d is used to to calculate the JOINT estimations with
                the help of Computer Vision and Deep Learning.
                <br />
                <a href="https://github.com/facebookresearch/VideoPose3D">
                  More
                </a>
              </Typography>
            </Grid>
            <Grid item xs={3}>
              <Typography variant="h6">
                .NET 7
              </Typography>
              <Typography variant="subtitle2">
                .NET 7 is used to connect all of the processes needed for the
                estimation of poses
                <br />
                <a href="https://dotnet.microsoft.com/en-us/download/dotnet/7.0">
                  More
                </a>
              </Typography>
            </Grid>
            <Grid item xs={3}>
              <Typography variant="h6">
                RavenDB
              </Typography>
              <Typography variant="subtitle2">
                RavenDb is used for efficient and modern storing of data
                <br />
                <a href="https://ravendb.net/">More</a>
              </Typography>
            </Grid>
          </Grid>
        </Grid>
      </Grid>
    </>
  );
};

export default Home;
