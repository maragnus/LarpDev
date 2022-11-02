import React from 'react';
import logo from './logo.svg';
import './App.css';
import {larpAuthClient} from "./LarpService";
import {InitiateLoginRequest, InitiateLoginResponse} from "./Protos/larp/authorization_pb";

function App() {
  let a = new InitiateLoginRequest();
  a.setEmail("acrion@gmail.com");
  larpAuthClient.initiateLogin(a, null, (err: any, response?: InitiateLoginResponse) => {
    console.log(err);
    console.log(response?.toObject());
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
