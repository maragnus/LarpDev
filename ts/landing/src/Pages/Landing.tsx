import logo from '../logo.webp';
import splash from '../splash.webp';
import {Box} from "@mui/material";
import {Copyright} from "../Common/Copyright";

export default function Landing() {
    return (
        <Box sx={{textAlign: "center"}}>
            <Box sx={{
                backgroundImage: `url(${splash})`,
                backgroundPosition: "center center",
                backgroundRepeat: "no-repeat",
                backgroundSize: "cover",
                position: 'absolute',
                left: 0, top: 0, right: 0, bottom: 0,
                zIndex: -1,
            }}>
                <Box sx={{position: "absolute", bottom: 0, left: 0, right: 0, mb: 8 }}>
                    <Copyright color="#cccccc"/>
                </Box>
            </Box>
            <img src={logo} alt="Mystwood Logo"
                 style={{maxWidth: "75%", height: "auto"}}/>
        </Box>
    );
}