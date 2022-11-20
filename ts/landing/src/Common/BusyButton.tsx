import {CircularProgress} from "@mui/material";
import * as React from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";

export function BusyButton(params: { label: string, $busy: boolean, sx?: any }) {
    return (
        <Box sx={{position: 'relative'}} {...params}>
            <Button
                type="submit"
                fullWidth
                variant="contained"
                disabled={params.$busy}
            >
                {params.label}
            </Button>
            {params.$busy && (<CircularProgress
                size={24}
                sx={{
                    position: 'absolute',
                    top: '50%',
                    left: '50%',
                    marginTop: '-12px',
                    marginLeft: '-12px',
                }}/>)}
        </Box>);
}