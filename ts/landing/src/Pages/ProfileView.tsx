import * as React from "react";
import {
    Alert,
    AlertTitle,
    Box,
    Button,
    Container,
    IconButton,
    List,
    ListItem,
    ListItemIcon,
    ListItemText,
    Tab,
    Tabs,
    Typography
} from "@mui/material";
import sessionService from "../SessionService";
import {useNavigate} from "react-router-dom";
import TextField from "@mui/material/TextField";
import DeleteIcon from '@mui/icons-material/Delete';
import StarIcon from '@mui/icons-material/Star';
import StarOutlineIcon from '@mui/icons-material/StarOutline';
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

interface AccountProps {
    account: Account.AsObject
    updateAccount: (account: Account.AsObject) => any;
}

function EditProfile(props: AccountProps) {
    const [busy, setBusy] = React.useState(false);

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        setBusy(true);

        let name: string = (data.get('name') as string ?? "").trim();
        let location: string = (data.get('location') as string ?? "").trim();
        let phone: string = (data.get('phone') as string ?? "").trim();
        let notes: string = (data.get('notes') as string ?? "").trim();

        try {
            const newAccount = await sessionService.setProfile(name, location, phone, notes);
            props.updateAccount(newAccount);
        } catch (e: any) {
            alert(e);
        } finally {
            setBusy(false);
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
                defaultValue={props.account.name}
            />
            <TextField
                margin="normal"
                required
                fullWidth
                id="location"
                label="Location"
                name="location"
                autoComplete="street-address"
                defaultValue={props.account.location}
            />
            <TextField
                margin="normal"
                required
                fullWidth
                id="phone"
                label="Phone Number"
                name="phone"
                autoComplete="tel"
                defaultValue={props.account.phone}
            />
            <TextField
                margin="normal"
                fullWidth
                id="notes"
                label="Notes"
                name="notes"
                defaultValue={props.account.notes}
            />

            <BusyButton label="Save" busy={busy} sx={{mt: 3, mb: 2}}/>
        </Box>
    );
}

function EditEmail(props: AccountProps) {
    const [busy, setBusy] = React.useState(false);

    async function remove(email: string) {
        setBusy(true);
        try {
            props.updateAccount(await sessionService.removeEmail(email));
        } catch (e: any) {
            alert(e);
        } finally {
            setBusy(false);
        }
    }


    async function prefer(email: string) {
        setBusy(true);
        try {
            props.updateAccount(await sessionService.preferredEmail(email));
        } catch (e: any) {
            alert(e);
        } finally {
            setBusy(false);
        }
    }

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        const data = new FormData(event.currentTarget);
        setBusy(true);

        let email: string = (data.get('email') as string ?? "").trim();

        try {
            props.updateAccount(await sessionService.addEmail(email));
        } catch (e: any) {
            alert(e);
        } finally {
            setBusy(false);
        }
    }

    const addresses = props.account.emailsList.map(email => (
        <ListItem>
            <ListItemText primary={email.email} secondary={email.isVerified ? "Verified" : "Unverified"}/>
            <ListItemIcon>
                <IconButton edge="end" aria-label="star" onClick={() => prefer(email.email)} disabled={busy}
                            color={email.isPreferred ? "success" : "primary"}>
                    {email.isPreferred ? <StarIcon/> : <StarOutlineIcon/>}
                </IconButton>
            </ListItemIcon>
            <ListItemIcon>
                <IconButton edge="end" aria-label="delete" onClick={() => remove(email.email)} disabled={busy}>
                    <DeleteIcon/>
                </IconButton>
            </ListItemIcon>
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
                    <EditProfile account={account} updateAccount={updateAccount}/>
                </TabPanel>
                <TabPanel value={value} index={1}>
                    <Typography variant="h5" sx={{my: 2}} align="center">Authorized Emails</Typography>
                    <EditEmail account={account} updateAccount={updateAccount}/>
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