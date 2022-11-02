import {CircularProgress} from "@mui/material";
import * as React from "react";
import Box from "@mui/material/Box";
import Button from "@mui/material/Button";

export function BusyButton(params: { label: string, busy: boolean, sx?: any }) {
    return (
        <Box {...params} sx={{m: 1, position: 'relative'}}>
            <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{mt: 3, mb: 2}}
                disabled={params.busy}
            >
                {params.label}
            </Button>
            {params.busy && (<CircularProgress
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