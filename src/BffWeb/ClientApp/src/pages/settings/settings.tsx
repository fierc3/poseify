import { Stack, Text } from "@fluentui/react";
import { FC } from "react";
import useClaims from "../../helpers/claims";

const Settings: FC = () => {
    const {data} = useClaims();

    return (
        <Stack>
            <Text variant="xLarge">Settings</Text>
            <Text variant="large">BFF Claims</Text>
            <Text variant="medium">{JSON.stringify(data)}</Text>
        </Stack>
    );
  }
  
  export default Settings;
  