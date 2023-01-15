import * as React from 'react';
import {useNavigate} from "react-router-dom";
import {useMountEffect} from "../UseMountEffect";
import {
    Avatar,
    Container, IconButton,
    List,
    ListItem,
    ListItemAvatar, ListItemIcon,
    ListItemText,
    ListSubheader,
    Typography
} from "@mui/material";
import AwesomeSpinner from "../../Common/AwesomeSpinner";
import {Event, EventComponent, EventRsvp} from "../../Protos/larp/events_pb";
import sessionService from "../../SessionService";
import ExtensionIcon from '@mui/icons-material/Extension';
import MenuItem from "@mui/material/MenuItem";
import RsvpIcon from "@mui/icons-material/Rsvp";

function ComponentItem(props: {key: number, event: Event.AsObject, component: EventComponent.AsObject, busy: boolean}) {
    const eventId = props.event.eventId;
    const isApproved = false;
    const name = props.component.name;
    const when = new Date(props.component.date).toDateString();

    function select(id: string, rsvp: EventRsvp) {
    }

    function handleClick() {

    }

    let rsvpOptions: any;
    if (isApproved) {
        rsvpOptions = undefined;
    } else if (false) {
        rsvpOptions = [
            <MenuItem onClick={() => select(eventId, EventRsvp.EVENT_RSVP_CONFIRMED)}>Yes, I attended</MenuItem>,
            <MenuItem onClick={() => select(eventId, EventRsvp.EVENT_RSVP_UNANSWERED)}>No, I did not attend</MenuItem>,
        ]
    } else {
        rsvpOptions = [
            <MenuItem onClick={() => select(eventId, EventRsvp.EVENT_RSVP_YES)}>Going</MenuItem>,
            <MenuItem onClick={() => select(eventId, EventRsvp.EVENT_RSVP_MAYBE)}>Maybe</MenuItem>,
            <MenuItem onClick={() => select(eventId, EventRsvp.EVENT_RSVP_NO)}>Not Going</MenuItem>
        ]
    }

    return (
        <ListItem key={props.key}>
            <ListItemAvatar><Avatar><ExtensionIcon/></Avatar></ListItemAvatar>
            <ListItemText primary={name} secondary={when}/>
            <ListItemIcon>
                <IconButton edge="end" aria-label="delete" onClick={handleClick} disabled={props.busy}>
                    <RsvpIcon/>
                </IconButton>
            </ListItemIcon>
        </ListItem>
    );
}

function EventViewItem(props: {event: Event.AsObject, busy: boolean}) {
    const ev = props.event;
    const components = ev.componentsList.map((c, i) =>
        (<ComponentItem key={i} event={ev} component={c} busy={props.busy} />));

    return <div>
        <Typography variant="subtitle1">{ev.location}</Typography>

        <List
            sx={{ width: '100%', maxWidth: 360, bgcolor: 'background.paper' }}
            subheader={<ListSubheader>Event Components</ListSubheader>}>
            {components}
        </List>
    </div>;
}

export default function EventViewPage(params: { id: string }) {
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
            {!busy && <EventViewItem event={event} busy={busy}/>}
        </Container>
    );
}