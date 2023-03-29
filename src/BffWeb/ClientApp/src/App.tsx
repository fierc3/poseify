import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';
import { Route, Routes } from 'react-router-dom';
import Dashboard from './pages/dashboard/Dashboard';
import Visitor from './pages/visitor/Visitor';
import { QueryClient, QueryClientProvider } from 'react-query';
import { ReactQueryDevtools } from 'react-query/devtools'

const App = () => {
  return (
    <div className="App">
      <QueryClientProvider client={new QueryClient()}>
        <Routes>
          <Route path="/" element={<Visitor />} />
          <Route path="/Dashboard" element={<Dashboard />} />
        </Routes>
        <ReactQueryDevtools initialIsOpen={true}></ReactQueryDevtools>
      </QueryClientProvider>
    </div>
  );
}

export default App;
