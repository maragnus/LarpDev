import {AccountProps} from "./AccountProps";
import {Event, EventRsvp} from "../../Protos/larp/events_pb";
import {Box, IconButton, List, ListItem, ListItemIcon, ListItemText, ListSubheader, Typography} from "@mui/material";
import * as React from "react";
import Menu from '@mui/material/Menu';
import MenuItem from '@mui/material/MenuItem';
import RsvpIcon from '@mui/icons-material/Rsvp';
import GoingIcon from '@mui/icons-material/ThumbUp';
import NotGoingIcon from '@mui/icons-material/ThumbDown';
import MaybeGoingIcon from '@mui/icons-material/ThumbsUpDown';
import ConfirmedIcon from '@mui/icons-material/Check';
import ApprovedIcon from '@mui/icons-material/Star';
import UnknownIcon from '@mui/icons-material/EventNote';
import sessionService from "../../SessionService";
import AwesomeSpinner from "../../Common/AwesomeSpinner";
import {useMountEffect} from "../UseMountEffect";

interface EventDetails {
    id: string;
    title: string;
    date: number;
    when: string;
    rsvp: EventRsvp;
    canRsvp: boolean;
    location: string;
    gameId: string;
    isPast: boolean;
}

function EventListItem(props: { event: EventDetails, busy: boolean, updateRsvp: (id: string, rsvp: EventRsvp) => void }) {
    const event = props.event;

    const isApproved = event.rsvp === EventRsvp.EVENT_RSVP_APPROVED;

    const [anchorEl, setAnchorEl] = React.useState<null | HTMLElement>(null);
    const open = Boolean(anchorEl);

    function handleClick(event: React.MouseEvent<HTMLElement>) {
        setAnchorEl(event.currentTarget);
    }

    function handleClose() {
        setAnchorEl(null);
    }

    function select(id: string, rsvp: EventRsvp) {
        setAnchorEl(null);
        props.updateRsvp(id, rsvp);
    }

    let icon: any;
    switch (event.rsvp) {
        case EventRsvp.EVENT_RSVP_NO:
            icon = <NotGoingIcon/>;
            break;
        case EventRsvp.EVENT_RSVP_MAYBE:
            icon = <MaybeGoingIcon/>;
            break;
        case EventRsvp.EVENT_RSVP_YES:
            icon = <GoingIcon/>;
            break;
        case EventRsvp.EVENT_RSVP_APPROVED:
            icon = <ApprovedIcon/>;
            break;
        case EventRsvp.EVENT_RSVP_CONFIRMED:
            icon = <ConfirmedIcon/>;
            break;
        case EventRsvp.EVENT_RSVP_UNANSWERED:
            icon = <UnknownIcon/>;
            break;
    }

    let rsvpOptions: any;
    if (isApproved) {
        rsvpOptions = undefined;
    } else if (event.isPast) {
        rsvpOptions = [
            <MenuItem onClick={() => select(event.id, EventRsvp.EVENT_RSVP_CONFIRMED)}>Yes, I attended</MenuItem>,
            <MenuItem onClick={() => select(event.id, EventRsvp.EVENT_RSVP_UNANSWERED)}>No, I did not attend</MenuItem>,
        ]
    } else {
        rsvpOptions = [
            <MenuItem onClick={() => select(event.id, EventRsvp.EVENT_RSVP_YES)}>Going</MenuItem>,
            <MenuItem onClick={() => select(event.id, EventRsvp.EVENT_RSVP_MAYBE)}>Maybe</MenuItem>,
            <MenuItem onClick={() => select(event.id, EventRsvp.EVENT_RSVP_NO)}>Not Going</MenuItem>
        ]
    }

    return (<ListItem key={event.date}>
        <ListItemIcon>
            {icon}
        </ListItemIcon>
        <ListItemText>
            <Typography component="span" variant="body2">{event.title}</Typography><br/>
            <Typography component="span" variant="caption">{event.when}</Typography>
        </ListItemText>

        {!isApproved && (
            <ListItemIcon>
                <IconButton edge="end" aria-label="delete" onClick={handleClick} disabled={props.busy}>
                    <RsvpIcon/>
                </IconButton>
            </ListItemIcon>
        )}

        {!isApproved && (
            <Menu
                id="demo-positioned-menu"
                aria-labelledby="demo-positioned-button"
                anchorEl={anchorEl}
                open={open}
                onClose={handleClose}
                anchorOrigin={{
                    vertical: 'top',
                    horizontal: 'left',
                }}
                transformOrigin={{
                    vertical: 'top',
                    horizontal: 'left',
                }}>
                {rsvpOptions}
            </Menu>
        )}
    </ListItem>);
}

export function EditAttendance(props: AccountProps): any {
    const [busy, setBusy] = React.useState(true);
    const [events, setEvents] = React.useState([] as Event.AsObject[]);

    async function refresh() {
        console.log('Refreshing event list for ' + props.account.name);
        try {
            const response = await sessionService.getEvents(true, true, true);
            setEvents(response);
        } catch (e) {
            alert('Failed to load events list: ' + e);
        } finally {
            setBusy(false);
        }
    }

    async function updateRsvp(id: string, rsvp: EventRsvp) {
        try {
            setBusy(true);
            await sessionService.rsvp(id, rsvp);
            await refresh();
        } catch (e) {
            alert('Failed to update RSVP:' + e);
        } finally {
            setBusy(false);
        }
    }

    useMountEffect(async () => {
        await refresh();
    });

    if (busy) {
        return <AwesomeSpinner/>;
    }

    const options: Intl.DateTimeFormatOptions = {
        weekday: 'long',
        year: 'numeric',
        month: 'long',
        day: 'numeric',
    };

    const eventList = events.map(event => {
        const attendee = event.attendeesList[0];
        return {
            id: event.eventId,
            title: event.title,
            date: Date.parse(event.date),
            when: new Date(event.date).toLocaleString('en-US', options),
            rsvp: attendee.rsvp,
            canRsvp: event.rsvp && attendee.rsvp !== EventRsvp.EVENT_RSVP_APPROVED,
            location: event.location,
            gameId: event.gameId,
            isPast: Date.parse(event.date) <= Date.now()
        } as EventDetails
    });

    const upcomingEvents = eventList
        .filter(event => event.isPast)
        .map(event => (<EventListItem event={event} busy={busy} updateRsvp={updateRsvp}/>));

    const pastEvents = eventList
        .filter(event => event.isPast)
        .map(event => (<EventListItem event={event} busy={busy} updateRsvp={updateRsvp}/>));

    const result =
        (<Box>
            <List
                sx={{width: '100%', maxWidth: 360, bgcolor: 'background.paper'}}
                component="nav"
                aria-labelledby="nested-list-subheader"
                subheader={
                    <ListSubheader component="div" id="nested-list-subheader">
                        Upcoming Events
                    </ListSubheader>
                }
            >
                {upcomingEvents}
            </List>

            <List
                sx={{width: '100%', maxWidth: 360, bgcolor: 'background.paper'}}
                component="nav"
                aria-labelledby="nested-list-subheader"
                subheader={
                    <ListSubheader component="div" id="nested-list-subheader">
                        Past Events
                    </ListSubheader>
                }
            >
                {pastEvents}
            </List>
        </Box>);

    return result;
}