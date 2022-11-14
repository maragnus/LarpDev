import * as React from 'react';
import BottomNavigation from '@mui/material/BottomNavigation';
import BottomNavigationAction from '@mui/material/BottomNavigationAction';
import {
    LockOpenOutlined,
    CalendarToday,
    GroupsOutlined,
    BookOutlined,
    PersonOutline,
    AccountBoxOutlined,
} from "@mui/icons-material";
import {NavLink} from 'react-router-dom';
import sessionService from "../SessionService";

export default class LandingNavigation extends React.Component<any, any> {
    constructor(props: any) {
        super(props);
        this.state = {
            value: 0,
            isAuthenticated: sessionService.isAuthenticated(),
            isAdmin: sessionService.isAdmin()
        };
    }

    private _sessionCallback?: number;

    componentDidMount() {
        this._sessionCallback = sessionService.subscribe(() => {
            this.setState((state: any) => ({
                value: state.value,
                isAuthenticated: sessionService.isAuthenticated(),
                isAdmin: sessionService.isAdmin()
            }));
        })
    }

    componentWillUnmount() {
        sessionService.unsubscribe(this._sessionCallback);
    }

    render() {
        let items: JSX.Element[] = [];

        if (this.state.isAuthenticated) {
            items.push(<BottomNavigationAction key={0} label="Profile" icon={<AccountBoxOutlined/>} to="/profile" component={NavLink}/>);
            items.push(<BottomNavigationAction key={1} label="Characters" icon={<GroupsOutlined/>} to="/characters" component={NavLink}/>);
        } else {
            items.push(<BottomNavigationAction key={3} label="Login" icon={<LockOpenOutlined/>} to="/login" component={NavLink}/>);
        }

        if (this.state.isAdmin) {
            items.push(<BottomNavigationAction key={4} label="Players" icon={<PersonOutline/>} to="/players/search" component={NavLink}/>);
        } else if (false) {
            items.push(<BottomNavigationAction key={5} label="Clarify" icon={<BookOutlined/>} to="/clarify" component={NavLink}/>);
        }

        return (
            <BottomNavigation
                showLabels
                value={this.state.value}
                onChange={(event, newValue) => {
                    this.setState({value: newValue});
                }}
            >
                {items}
                <BottomNavigationAction key={12} label="Events" icon={<CalendarToday/>} to="/events" component={NavLink}/>
            </BottomNavigation>
        );
    }
}