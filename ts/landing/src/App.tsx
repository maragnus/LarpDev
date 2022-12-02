import React from 'react';
import {Route, Routes, useParams} from 'react-router-dom';
import {Container, Paper} from "@mui/material";
import './App.css';
import Landing from "./Pages/Landing";
import LandingNavigation from "./Common/LandingNavigation";
import LoginPage from "./Pages/Authentication/LoginPage";
import ConfirmPage from "./Pages/Authentication/ConfirmPage";
import ProfilePage from "./Pages/Profile/ProfilePage";
import {useMountEffect} from "./Pages/UseMountEffect";
import sessionService from "./SessionService";
import AwesomeSpinner from "./Common/AwesomeSpinner";
import EventListPage from "./Pages/Events/EventListPage";
import CharactersPage from "./Pages/Characters/CharactersPage";
import EventViewPage from "./Pages/Events/EventViewPage";

function EventViewPageProxy() {
    const {eventId} = useParams();
    return (<EventViewPage id={eventId!}/>);
}

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
                <Route path="/events" element={<EventListPage/>}/>
                <Route path="/events/:eventId" element={<EventViewPageProxy />}/>
                <Route path="/characters" element={<CharactersPage/>}/>
            </Routes>
            <Paper sx={{position: 'fixed', bottom: 0, left: 0, right: 0, zIndex: 1000}} elevation={3}>
                <LandingNavigation/>
            </Paper>
        </Container>
    );
}

export default App;
