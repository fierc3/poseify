import React, { useState } from 'react';
import logo from './logo.svg';
import './App.css';
import {env} from './config';
import { getCurrentUserProfile } from './helpers/api';
import { IDefaultReturn } from './helpers/api.types';

function App() {

  const [userProfile, setUserProfile] = useState<IDefaultReturn>()

  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Current Backend: {env.backend.baseUrl}
        </p>
        <p>{JSON.stringify(userProfile)}</p>

          <button onClick={async () => {setUserProfile((await getCurrentUserProfile()))}}>Fetch Test User</button>
      </header>
    </div>
  );
}

export default App;
