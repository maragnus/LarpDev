import Typography from "@mui/material/Typography";
import Link from "@mui/material/Link";
import * as React from "react";

export function Copyright(props: any) {
    return (
        <Typography variant="body2" color="text.secondary" align="center" {...props}>
            {'Copyright © '}
            {new Date().getFullYear()}
            {' '}
            <Link color="inherit" href="http://mystwood.org/" target="_blank">
                Mystwood
            </Link>
            <br/>
            {'Mystwood Landing by '}
            <Link color="inherit" href="https://github.com/maragnus/Mystwood.Landing" target="_blank">
                Josh Brown
            </Link>
        </Typography>
    );
}