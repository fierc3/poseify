import React from 'react';
import './app.css';
import { Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools'
import DefaultNavbar from './components/navbar/default-navbar';
import 'bootstrap/dist/css/bootstrap.min.css';
import AuthUserLayout from './components/guards/auth-user-layout';
import { loggedInRoutes, publicRoutes } from './helpers/routes';
import { initializeIcons, PartialTheme, ThemeProvider } from '@fluentui/react';

const App = () => {

  initializeIcons();

  const appTheme: PartialTheme = ({
    palette: {
      themePrimary: '#cb47ff',
      themeLighterAlt: '#08030a',
      themeLighter: '#200b29',
      themeLight: '#3d154d',
      themeTertiary: '#7a2b99',
      themeSecondary: '#b33fe0',
      themeDarkAlt: '#d05aff',
      themeDark: '#d773ff',
      themeDarker: '#e298ff',
      neutralLighterAlt: '#000000',
      neutralLighter: '#000000',
      neutralLight: '#000000',
      neutralQuaternaryAlt: '#000000',
      neutralQuaternary: '#000000',
      neutralTertiaryAlt: '#000000',
      neutralTertiary: '#f1f0ee',
      neutralSecondary: '#f4f2f1',
      neutralPrimaryAlt: '#f6f5f4',
      neutralPrimary: '#ebe8e6',
      neutralDark: '#fafaf9',
      black: '#fdfcfc',
      white: '#000000',
    }});

  return (
    <div className="App">
      <ThemeProvider theme={appTheme}>
      <QueryClientProvider client={new QueryClient()}>
      <DefaultNavbar/>
        <Routes>
          {publicRoutes}
          <Route element={<AuthUserLayout/>}>
            {loggedInRoutes}
          </Route>
        </Routes>
        <ReactQueryDevtools initialIsOpen={false}></ReactQueryDevtools>
      </QueryClientProvider>
      </ThemeProvider>
    </div>
  );
}

export default App;
