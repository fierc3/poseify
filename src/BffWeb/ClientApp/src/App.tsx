import "./App.css";
import { Route, Routes } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "react-query";
import { ReactQueryDevtools } from "react-query/devtools";
import DefaultNavbar from "./components/navbar/default-navbar";
import "bootstrap/dist/css/bootstrap.min.css";
import AuthUserLayout from "./components/guards/auth-user-layout";
import { loggedInRoutes, publicRoutes } from "./helpers/routes";
import { ThemeProvider } from '@mui/material/styles';
import defaultTheme from "./themes/default-theme";

const App = () => {
  return (
    <div className="App">
      <QueryClientProvider client={new QueryClient()}>
        <ThemeProvider theme={defaultTheme}>
          <DefaultNavbar />
          <Routes>
            {publicRoutes}
            <Route element={<AuthUserLayout />}>{loggedInRoutes}</Route>
          </Routes>
        </ThemeProvider>
      </QueryClientProvider>
    </div>
  );
};

export default App;
