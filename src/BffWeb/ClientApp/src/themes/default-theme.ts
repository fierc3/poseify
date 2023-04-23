import { createTheme } from "@mui/material";

const theme = createTheme({
  palette: {
    primary: {
      main: '#000000',
    },
    secondary: {
      main: '#8e1872',
    },
  }
});
export default theme
// Then, pass it to `<CssVarsProvider theme={theme}>`.
