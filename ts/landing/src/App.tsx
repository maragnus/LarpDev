import React from 'react';
import logo from './logo.svg';
import './App.css';
import {larpAuthClient} from "./LarpService";
import * as larp_authorization_pb from './Protos/larp/authorization_pb.d';

function App() {
  let a = new larp_authorization_pb.InitiateLoginRequest();
  a.setEmail("acrion@gmail.com");
  larpAuthClient.initiateLogin(a, null, function(err, response) {
    console.log(response.toObject());
  })
  return (
    <div className="App">
      <header className="App-header">
        <img src={logo} className="App-logo" alt="logo" />
        <p>
          Edit <code>src/App.tsx</code> and save to reload.
        </p>
        <a
          className="App-link"
          href="https://reactjs.org"
          target="_blank"
          rel="noopener noreferrer"
        >
          Learn React
        </a>
      </header>
    </div>
  );
}

export default App;
