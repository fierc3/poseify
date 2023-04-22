import { ProgressIndicator, Stack, ThemeProvider } from "@fluentui/react";
import { Text } from "@fluentui/react/lib/Text";
import { EstimationList } from "../../components/estimation-list/estimation-list";
import { navigationDarkTheme } from "../../themes/navigation-theme-dark";

const Dashboard = () => {
  return (
    <div className="App">
      <Stack horizontal>
        <Stack.Item grow={5}>
          <Stack
            horizontalAlign="center"
            verticalAlign="space-evenly"
            style={{ minHeight: 300 }}
          >
            <Stack.Item>
              <Text variant="mega">Place holder</Text>
            </Stack.Item>
          </Stack>
          <Stack>
            <EstimationList />
          </Stack>
        </Stack.Item>
        <Stack.Item grow={1}>
          <ThemeProvider theme={navigationDarkTheme}>
            <Stack
              verticalAlign="start"
              style={{ width: 200, height: "87vh" }}
            >
              <Stack horizontal>
                <Stack.Item
                  grow={1}
                  style={{ paddingLeft: 10, paddingRight: 10 }}
                >
                  <ProgressIndicator
                    styles={{ progressTrack: { backgroundColor: "white" } }}
                    label="Daily Limit"
                    description="Used x out of 10"
                    percentComplete={0.5}
                  />
                </Stack.Item>
              </Stack>
            </Stack>
          </ThemeProvider>
        </Stack.Item>
      </Stack>
    </div>
  );
};

export default Dashboard;
