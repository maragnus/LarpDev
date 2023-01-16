import * as React from "react";
import {Alert, AlertTitle, Box, Button, Container, Tab, Tabs, Typography} from "@mui/material";
import sessionService from "../../SessionService";
import {useNavigate} from "react-router-dom";
import {useMountEffect} from "../UseMountEffect";
import AwesomeSpinner from "../../Common/AwesomeSpinner";
import {Account} from "../../Protos/larp/accounts_pb";
import {EditProfile} from "./EditProfile";
import {EditEmail} from "./EditEmail";
import {EditAttendance} from "./EditAttendance";

function TabPanel(props: any) {
    const {children, value, index, ...other} = props;

    return (
        <div
            role="tabpanel"
            hidden={value !== index}
            id={`vertical-tabpanel-${index}`}
            aria-labelledby={`vertical-tab-${index}`}
            {...other}
        >
            {value === index && (
                <Box sx={{p: 3}}>
                    {children}
                </Box>
            )}
        </div>
    );
}

function tabProps(index: number) {
    return {
        id: `vertical-tab-${index}`,
        'aria-controls': `vertical-tabpanel-${index}`
    };
}

export default function ProfilePage() {
    let navigate = useNavigate();
    const [busy, setBusy] = React.useState(true);
    const [value, setValue] = React.useState(0);
    const [account, setAccount] = React.useState((new Account()).toObject(true));

    if (!sessionService.isAuthenticated())
        navigate("/");

    const handleChange = (event: any, newValue: number) => {
        setValue(newValue);
    };

    async function logout() {
        await sessionService.logout();
        navigate("/");
    }

    useMountEffect(async () => {
        setAccount(await sessionService.getAccount());
        setBusy(false);
    });

    function updateAccount(account: Account.AsObject) {
        setAccount(account);
    }

    if (busy) {
        return <Container maxWidth="md">
            <Typography variant="h4" sx={{mt: 2}} align="center">Your Profile</Typography>
            <AwesomeSpinner/>
        </Container>
    }

    return (
        <Container maxWidth="md">
            <Typography variant="h4" sx={{mt: 2}} align="center">Your Profile</Typography>
            <Box sx={{flexGrow: 1, bgcolor: 'background.paper', display: 'flex'}}>
                <Tabs
                    orientation="vertical"
                    variant="scrollable"
                    value={value}
                    onChange={handleChange}
                    aria-label="Vertical tabs example"
                    sx={{borderRight: 1, borderColor: 'divider', minWidth: '33%'}}
                >
                    <Tab label="Profile" {...tabProps(0)} />
                    <Tab label="Email" {...tabProps(1)} />
                    <Tab label="Attendance" {...tabProps(2)} />
                    <Tab label="Two-factor" {...tabProps(3)} />
                    <Tab label="Personal data" {...tabProps(4)} />
                    <Tab label="Logout" {...tabProps(5)} />
                </Tabs>
                <TabPanel value={value} index={0}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Personal Profile</Typography>
                    <EditProfile account={account} updateAccount={updateAccount}/>
                </TabPanel>
                <TabPanel value={value} index={1}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Authorized Emails</Typography>
                    <EditEmail account={account} updateAccount={updateAccount}/>
                </TabPanel>
                <TabPanel value={value} index={2}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Event Attendance</Typography>
                    <EditAttendance account={account} updateAccount={updateAccount}/>
                </TabPanel>
                <TabPanel value={value} index={3}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Two-factor</Typography>

                    <Alert severity="warning">
                        <AlertTitle>Not Implemented</AlertTitle>
                        Currently, this application only supports <strong>email token authentication</strong>.
                        Passwords, external providers, and external security tokens will be implemented in the future.
                    </Alert>
                </TabPanel>
                <TabPanel value={value} index={4}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Personal data</Typography>

                    <Alert severity="warning">
                        <AlertTitle>Not Implemented</AlertTitle>
                        <p>The only personal information stored is available in the Profile and Email sections. A data
                            download of this information will be made available in the future.</p>
                        <p>No hidden advertising or demographic data is recorded about you or your session.</p>
                    </Alert>
                </TabPanel>
                <TabPanel value={value} index={5}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Sign out on this device</Typography>

                    <Button variant="contained" onClick={logout} sx={{mt: 2, mx: 1}}>Logout</Button>
                </TabPanel>
            </Box>
        </Container>
    );
}