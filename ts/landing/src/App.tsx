import React from 'react';
import {Route, Routes, useNavigate, useParams} from 'react-router-dom';
import {Container, Paper} from "@mui/material";
import './App.css';
import Landing from "./Pages/Landing";
import LandingNavigation from "./Common/LandingNavigation";
import LoginPage from "./Pages/LoginPage";
import ConfirmPage from "./Pages/ConfirmPage";
import ProfileView from "./Pages/ProfileView";

function App() {
  return (
      <Container sx={{pb: 7}} maxWidth="xl" className="App">
        <Routes>
            <Route path="/" element={<Landing/>}/>
            <Route path="/login" element={<LoginPage/>}/>
            <Route path="/confirm" element={<ConfirmPage/>}/>
            <Route path="/profile" element={<ProfileView/>}/>
        </Routes>
          <Paper sx={{position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1000}} elevation={3}>
              <LandingNavigation/>
          </Paper>
      </Container>
  );
}

export default App;
