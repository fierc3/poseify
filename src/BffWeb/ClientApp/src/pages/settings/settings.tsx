import { FC } from "react";
import useClaims from "../../helpers/claims";
import { Box, Typography } from "@mui/material";

const Settings: FC = () => {
    const {data: claims} = useClaims();
    const {data: identity} = useAuthentication();

    return (
        <Box sx={{ my: 3, mx: 2 }}>
            <Typography variant="h2">Settings</Typography>
            <Typography variant="subtitle1">BFF Claims</Typography>
            <Typography variant="body2">{JSON.stringify(data)}</Typography>
            <Typography variant="body2">{JSON.stringify(identity)}</Typography>
        </Box>
    );
  }
  
  export default Settings;
  