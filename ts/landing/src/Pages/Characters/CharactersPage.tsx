import {useNavigate} from "react-router-dom";
import * as React from "react";
import {useMountEffect} from "../UseMountEffect";
import sessionService from "../../SessionService";
import {Container, List, ListItem, Typography} from "@mui/material";
import AwesomeSpinner from "../../Common/AwesomeSpinner";
import {Character} from "../../Protos/larp/mw5e/character";

function CharacterListItem(props: {key: number, character: Character}) {
    return (<ListItem key={props.key}>
        {props.character.characterName}
    </ListItem>);
}

function CharacterItems(props: {characters: Character[]}) {
    const items = props.characters
        .map((character, index)=>
            <CharacterListItem key={index} character={character}/>);

    return (<List>{items}</List>);
}

export default function CharactersPage() {
    const navigate = useNavigate();
    const [busy, setBusy] = React.useState(true);
    const [characters, setCharacters] = React.useState<Character[]>([]);

    useMountEffect(async () => {
        try {
            const events = await sessionService.getCharacters();
            setCharacters(events);
        } catch (e) {
            alert(e);
            navigate("/login");
        }
        setBusy(false);
    });

    return (
        <Container maxWidth="sm">
            <Typography variant="h4" sx={{mt: 2}} align="center">Characters</Typography>
            {busy && <AwesomeSpinner/>}
            {!busy && <CharacterItems characters={characters}/>}
        </Container>
    );
}