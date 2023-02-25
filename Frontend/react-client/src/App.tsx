import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';
import {env} from './config';
import { getCurrentUserProfile } from './helpers/api';
import { IDefaultReturn } from './helpers/api.types';
import { Route, Routes } from 'react-router-dom';
import Dashboard from './pages/dashboard/Dashboard';
import Visitor from './pages/visitor/Visitor';

const App = () => {
  return (
    <div className="App">
      <Routes>
        <Route path="/" element={<Visitor/>}/>
        <Route path="/Dashboard" element={<Dashboard/>}/>
      </Routes>
    </div>
  );
}

export default App;
