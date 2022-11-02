import React from 'react';
import {Route, Routes, useNavigate, useParams} from 'react-router-dom';
import {Container, Paper} from "@mui/material";
import './App.css';
import {larpAuthClient} from "./LarpService";
import {InitiateLoginRequest, InitiateLoginResponse} from "./Protos/larp/authorization_pb";
import Landing from "./Pages/Landing";
import LandingNavigation from "./Common/LandingNavigation";
import LoginPage from "./Pages/LoginPage";

function App() {
  let a = new InitiateLoginRequest();
  a.setEmail("acrion@gmail.com");
  larpAuthClient.initiateLogin(a, null, (err: any, response?: InitiateLoginResponse) => {
    console.log(err);
    console.log(response?.toObject());
  })
  return (
      <Container sx={{pb: 7}} maxWidth="xl" className="App">
        <Routes>
            <Route path="/" element={<Landing/>}/>
            <Route path="/login" element={<LoginPage/>}/>
        </Routes>
          <Paper sx={{position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1000}} elevation={3}>
              <LandingNavigation/>
          </Paper>
      </Container>
  );
}

export default App;
