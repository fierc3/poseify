import { Stack } from "@fluentui/react";
import { FC } from "react";
import { Text, ITextProps } from "@fluentui/react/lib/Text";

const Home: FC = () => {
  const maxedContentItemStyle: React.CSSProperties = {
    maxWidth: 900,
  };

  const poweredContent: React.CSSProperties = {
    height: "100%",
    justifyContent: "center",
    width: 200,
  };

  return (
    <>
      <Stack verticalAlign="space-around" style={{ height: "100%" }}>
        <Stack
          horizontalAlign="center"
          verticalAlign="space-evenly"
          style={{ minHeight: 300 }}
        >
          <Stack.Item>
            <Text variant="mega">Welcome to POSEIFY</Text>
          </Stack.Item>
          <Stack.Item style={maxedContentItemStyle}>
            <Text variant="medium">
              Welcome to our video pose estimation service! We are excited to
              introduce you to our innovative solution that makes it easier than
              ever to analyze and understand human movement.
            </Text>
            <br />
            <Text variant="medium">
              Our service is designed to help professionals in a wide range of
              industries, from sports and fitness to healthcare and
              entertainment. Whether you're a coach looking to improve your
              athletes' form, a doctor monitoring a patient's physical therapy
              progress, or a filmmaker creating stunning action scenes, our
              technology can help you achieve your goals.
            </Text>
          </Stack.Item>
        </Stack>
        <Stack>
          <Text style={{ paddingBottom: 50, paddingTop: 50 }} variant="xLarge">
            POSEIFY is powered by
          </Text>
          <Stack horizontal wrap horizontalAlign="space-evenly">
            <Stack.Item style={poweredContent}>
              <Text block variant="large">
                VideoPose3d
              </Text>
              <Text variant="medium">
                VideoPose3d is used to to calculate the JOINT estimations with
                the help of Computer Vision and Deep Learning.
                <br />
                <a href="https://github.com/facebookresearch/VideoPose3D">
                  More
                </a>
              </Text>
            </Stack.Item>
            <Stack.Item style={poweredContent}>
              <Text block variant="large">
                .NET 7
              </Text>
              <Text variant="medium">
                .NET 7 is used to connect all of the processes needed for the
                estimation of poses
                <br />
                <a href="https://dotnet.microsoft.com/en-us/download/dotnet/7.0">
                  More
                </a>
              </Text>
            </Stack.Item>
            <Stack.Item style={poweredContent}>
              <Text block variant="large">
                RavenDB
              </Text>
              <Text variant="medium">
                RavenDb is used for efficient and modern storing of data
                <br />
                <a href="https://ravendb.net/">More</a>
              </Text>
            </Stack.Item>
          </Stack>
        </Stack>
      </Stack>
    </>
  );
};

export default Home;
