import {AccountProps} from "./AccountProps";
import * as React from "react";
import sessionService from "../../SessionService";
import {Box} from "@mui/material";
import TextField from "@mui/material/TextField";
import {BusyButton} from "../../Common/BusyButton";

export function EditProfile(props: AccountProps) {
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
            const newAccount = await sessionService.setProfile(name, phone, location, notes);
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

            <BusyButton label="Save" $busy={busy} sx={{mt: 3, mb: 2}}/>
        </Box>
    );
}