import "./App.css";
import { Route, Routes } from "react-router-dom";
import { QueryClient, QueryClientProvider } from "react-query";
import { ReactQueryDevtools } from "react-query/devtools";
import DefaultNavbar from "./components/navbar/default-navbar";
import "bootstrap/dist/css/bootstrap.min.css";
import AuthUserLayout from "./components/guards/auth-user-layout";
import { loggedInRoutes, publicRoutes } from "./helpers/routes";
import { initializeIcons, ThemeProvider } from "@fluentui/react";
import { contentDarkTheme } from "./themes/content-theme-dark";
import { navigationDarkTheme } from "./themes/navigation-theme-dark";

const App = () => {
  initializeIcons();

  return (
    <div className="App">
      <QueryClientProvider client={new QueryClient()}>
        <ThemeProvider theme={navigationDarkTheme}>
          <DefaultNavbar />
        </ThemeProvider>
        <ThemeProvider theme={contentDarkTheme}>
          <Routes>
            {publicRoutes}
            <Route element={<AuthUserLayout />}>{loggedInRoutes}</Route>
          </Routes>
          <ReactQueryDevtools initialIsOpen={false}></ReactQueryDevtools>
        </ThemeProvider>
      </QueryClientProvider>
    </div>
  );
};

export default App;
