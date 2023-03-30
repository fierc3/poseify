import React, { useState } from 'react';
import './App.css';
import { Route, Routes } from 'react-router-dom';
import { QueryClient, QueryClientProvider } from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools'
import useClaims from './helpers/claims';
import DefaultNavbar from './components/navbar/default-navbar';
import 'bootstrap/dist/css/bootstrap.min.css';
import Home from './pages/home/home';
import Dashboard from './pages/dashboard/dashboard';

const App = () => {
  return (
    <div className="App">
      <QueryClientProvider client={new QueryClient()}>
      <DefaultNavbar></DefaultNavbar>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/Dashboard" element={<Dashboard />} />
        </Routes>
        <ReactQueryDevtools initialIsOpen={true}></ReactQueryDevtools>
      </QueryClientProvider>
    </div>
  );
}

export default App;
