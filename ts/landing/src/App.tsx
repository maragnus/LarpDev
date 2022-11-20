import React from 'react';
import {Route, Routes} from 'react-router-dom';
import {Container, Paper} from "@mui/material";
import './App.css';
import Landing from "./Pages/Landing";
import LandingNavigation from "./Common/LandingNavigation";
import LoginPage from "./Pages/LoginPage";
import ConfirmPage from "./Pages/ConfirmPage";
import ProfilePage from "./Pages/ProfilePage";
import {useMountEffect} from "./Pages/UseMountEffect";
import sessionService from "./SessionService";
import AwesomeSpinner from "./Common/AwesomeSpinner";

function App() {
    const [busy, setBusy] = React.useState(true);

    useMountEffect(async () => {
        await sessionService.getGameState();
        setBusy(false);
    });

    if (busy) {
        return  <Container sx={{pb: 7}} maxWidth="xl" className="App">
            <Landing/>
            <AwesomeSpinner/>
        </Container>
    }

    return (
        <Container sx={{pb: 7}} maxWidth="xl" className="App">
            <Routes>
                <Route path="/" element={<Landing/>}/>
                <Route path="/login" element={<LoginPage/>}/>
                <Route path="/confirm" element={<ConfirmPage/>}/>
                <Route path="/profile" element={<ProfilePage/>}/>
            </Routes>
            <Paper sx={{position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1000}} elevation={3}>
                <LandingNavigation/>
            </Paper>
        </Container>
    );
}

export default App;
