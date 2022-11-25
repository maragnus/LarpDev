import * as React from 'react';
import {useNavigate} from "react-router-dom";
import {useMountEffect} from "./UseMountEffect";
import {Container, Typography} from "@mui/material";
import AwesomeSpinner from "../Common/AwesomeSpinner";
import {Event} from "../Protos/larp/events_pb";
import sessionService from "../SessionService";

function EventViewItem(params: {event: Event.AsObject}) {
    const ev = params.event;
    return <div>
        <Typography variant="subtitle1">{ev.location}</Typography>
    </div>;
}

export default function EventView(params: { id: number }) {
    const navigate = useNavigate();
    const [busy, setBusy] = React.useState(true);
    const [event, setEvent] = React.useState<Event.AsObject>({} as Event.AsObject);

    useMountEffect(async () => {
        try {
            const event = await sessionService.getEvent(params.id);
            setEvent(event);
        } catch (e) {
            alert(e);
            navigate("/login");
        }
        setBusy(false);
    });

    return (
        <Container maxWidth="xl">
            <Typography variant="h4" sx={{mt: 2}} align="center">{busy ? "Loading Event..." : event.title}</Typography>
            {busy && <AwesomeSpinner/>}
            {!busy && <EventViewItem event={event}/>}
        </Container>
    );
}