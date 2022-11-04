import * as React from "react";
import {
    Box,
    Tabs,
    Tab,
    Typography,
    Container,
    Button,
    List,
    ListItem,
    IconButton,
    ListItemText,
    Alert, AlertTitle
} from "@mui/material";
import sessionService from "../SessionService";
import {useNavigate} from "react-router-dom";
import TextField from "@mui/material/TextField";
import DeleteIcon from '@mui/icons-material/Delete';
import {BusyButton} from "../Common/BusyButton";
import {useMountEffect} from "./UseMountEffect";
import AwesomeSpinner from "../Common/AwesomeSpinner";
import {Account} from "../Protos/larp/accounts_pb";

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
                    <Typography>{children}</Typography>
                </Box>
            )}
        </div>
    );
}

function tabProps(index: number) {
    return {
        id: `vertical-tab-${index}`,
        'aria-controls': `vertical-tabpanel-${index}`,
    };
}

/*
function EditProfile() {
    const [busy, setBusy] = React.useState(false);
    const [profile, setProfile] = React.useState(new Account().toObject);

    //let profile = await sessionService.getAccount();
    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        setBusy(true);

        let name: string = (data.get('name') as string ?? "").trim();
        let location: string = (data.get('location') as string ?? "").trim();
        let phone: string = (data.get('phone') as string ?? "").trim();

        try {
            //await sessionService.setName(name);
            //await sessionService.setLocation(location);
            //await sessionService.setPhone(phone);
        } catch (e: any) {
            alert(e);
        } finally {
            setBusy(false);
            //profile = await sessionService.getAccount();
        }
    }

    return (
        <Box component="form" onSubmit={handleSubmit} noValidate sx={{mt: 1}}>
            <TextField
                margin="normal"
                required
                fullWidth
                id="name"
                label="Full Name"
                name="name"
                autoComplete="name"
                defaultValue={profile.name}
            />
            <TextField
                margin="normal"
                required
                fullWidth
                id="location"
                label="Location"
                name="location"
                autoComplete="street-address"
                defaultValue={profile.location}
            />
            <TextField
                margin="normal"
                required
                fullWidth
                id="phone"
                label="Phone Number"
                name="phone"
                autoComplete="tel"
                defaultValue={profile.phone}
            />

            <BusyButton label="Save" busy={busy} sx={{mt: 3, mb: 2}}/>
        </Box>
    );
}


function EditEmail() {
    const [busy, setBusy] = React.useState(false);
    const [profile, setProfile] = React.useState(new Account().toObject);

    async function remove(email: string) {
        setBusy(true);
        try {
            //await sessionService.removeEmail(email);
        } catch (e: any) {
            alert(e);
        }
        finally {
            setBusy(false);
            setProfile(await sessionService.getAccount());
        }
    }

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        setBusy(true);

        let email: string = (data.get('email') as string ?? "").trim();

        try {
            //await sessionService.addEmail(email);
        } catch (e: any) {
            alert(e);
        }
        finally {
            setBusy(false);
            setProfile(await sessionService.getAccount());
        }
    }

    const addresses = profile.emailsList.map(email => (
        <ListItem
            secondaryAction={
                <IconButton edge="end" aria-label="delete" onClick={() => remove(email.email)} disabled={busy}>
                    <DeleteIcon/>
                </IconButton>
            }
        >
            <ListItemText primary={email.email} secondary="Unverified"/>
        </ListItem>
    ));

    return (
        <Box>
            <List>
                {addresses}
            </List>

            <Box component="form" onSubmit={handleSubmit} noValidate sx={{mt: 1}}>
                <TextField
                    margin="normal"
                    required
                    fullWidth
                    id="email"
                    label="Add Email Address"
                    name="email"
                    autoComplete="email"
                />
                <BusyButton label="Add" busy={busy} sx={{mt: 3, mb: 2}} />
            </Box>
        </Box>
    );
}
*/

export default function ProfileView() {
    let navigate = useNavigate();
    const [busy, setBusy] = React.useState(true);
    const [value, setValue] = React.useState(0);
    const [account, setAccount] = React.useState((new Account()).toObject(true));

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
                    sx={{borderRight: 1, borderColor: 'divider'}}
                >
                    <Tab label="Profile" {...tabProps(0)} />
                    <Tab label="Email" {...tabProps(1)} />
                    <Tab label="Attendance" {...tabProps(2)} />
                    <Tab label="Two-factor" {...tabProps(3)} />
                    <Tab label="Personal data" {...tabProps(4)} />
                    <Button variant="contained" onClick={logout} sx={{mt: 2, mx: 1}}>Logout</Button>
                </Tabs>
                <TabPanel value={value} index={0}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Personal Profile</Typography>


                </TabPanel>
                <TabPanel value={value} index={1}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Authorized Emails</Typography>


                </TabPanel>
                <TabPanel value={value} index={2}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Event Attendance</Typography>

                    <Alert severity="warning">
                        <AlertTitle>Not Implemented</AlertTitle>
                        This area has not been implemented yet.
                    </Alert>
                </TabPanel>
                <TabPanel value={value} index={3}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Two-factor</Typography>

                    <Alert severity="warning">
                        <AlertTitle>Not Implemented</AlertTitle>
                        This area has not been implemented yet.
                    </Alert>
                </TabPanel>
                <TabPanel value={value} index={4}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Personal data</Typography>

                    <Alert severity="warning">
                        <AlertTitle>Not Implemented</AlertTitle>
                        This area has not been implemented yet.
                    </Alert>
                </TabPanel>
            </Box>
        </Container>
    );
}