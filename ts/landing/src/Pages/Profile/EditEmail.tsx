import {AccountProps} from "./AccountProps";
import * as React from "react";
import sessionService from "../../SessionService";
import {Box, IconButton, List, ListItem, ListItemIcon, ListItemText} from "@mui/material";
import StarIcon from "@mui/icons-material/Star";
import StarOutlineIcon from "@mui/icons-material/StarOutline";
import DeleteIcon from "@mui/icons-material/Delete";
import TextField from "@mui/material/TextField";
import {BusyButton} from "../../Common/BusyButton";

export function EditEmail(props: AccountProps) {
    const [busy, setBusy] = React.useState(false);
    const [addEmail, setAddEmail] = React.useState('');

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
        setBusy(true);

        //const data = new FormData(event.currentTarget);
        //let email: string = (data.get('email') as string ?? "").trim();

        try {
            props.updateAccount(await sessionService.addEmail(addEmail));
            setAddEmail('');
        } catch (e: any) {
            alert(e);
        } finally {
            setBusy(false);
        }
    }

    function handleChange(event: any) {
        setAddEmail(event.target.value)
    }

    const addresses = props.account.emails.map((email, index) => (
        <ListItem key={index}>
            <ListItemText primary={email.email} secondary={email.isVerified ? "Verified" : "Unverified"}/>
            <ListItemIcon sx={{m: 0, p: 0, mx: 1, minWidth: 0}}>
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
                    value={addEmail}
                    onChange={handleChange}
                />
                <BusyButton label="Add" $busy={busy} sx={{m: 0, mt: 0, mb: 2}}/>
            </Box>
        </Box>
    );
}