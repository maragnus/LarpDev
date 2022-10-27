import * as jspb from 'google-protobuf'

import * as larp_mw5e_character_pb from '../../larp/mw5e/character_pb';
import * as larp_mw5e_gifts_pb from '../../larp/mw5e/gifts_pb';
import * as larp_mw5e_occupations_pb from '../../larp/mw5e/occupations_pb';
import * as larp_mw5e_other_pb from '../../larp/mw5e/other_pb';
import * as larp_mw5e_skills_pb from '../../larp/mw5e/skills_pb';


export class GameState extends jspb.Message {
  getGiftsList(): Array<larp_mw5e_gifts_pb.Gift>;
  setGiftsList(value: Array<larp_mw5e_gifts_pb.Gift>): GameState;
  clearGiftsList(): GameState;
  addGifts(value?: larp_mw5e_gifts_pb.Gift, index?: number): larp_mw5e_gifts_pb.Gift;

  getSkillsList(): Array<larp_mw5e_skills_pb.SkillDefinition>;
  setSkillsList(value: Array<larp_mw5e_skills_pb.SkillDefinition>): GameState;
  clearSkillsList(): GameState;
  addSkills(value?: larp_mw5e_skills_pb.SkillDefinition, index?: number): larp_mw5e_skills_pb.SkillDefinition;

  getOccupationsList(): Array<larp_mw5e_occupations_pb.Occupation>;
  setOccupationsList(value: Array<larp_mw5e_occupations_pb.Occupation>): GameState;
  clearOccupationsList(): GameState;
  addOccupations(value?: larp_mw5e_occupations_pb.Occupation, index?: number): larp_mw5e_occupations_pb.Occupation;

  getVantagesList(): Array<larp_mw5e_other_pb.Vantage>;
  setVantagesList(value: Array<larp_mw5e_other_pb.Vantage>): GameState;
  clearVantagesList(): GameState;
  addVantages(value?: larp_mw5e_other_pb.Vantage, index?: number): larp_mw5e_other_pb.Vantage;

  getReligionsList(): Array<larp_mw5e_other_pb.Religion>;
  setReligionsList(value: Array<larp_mw5e_other_pb.Religion>): GameState;
  clearReligionsList(): GameState;
  addReligions(value?: larp_mw5e_other_pb.Religion, index?: number): larp_mw5e_other_pb.Religion;

  getHomeChaptersList(): Array<larp_mw5e_other_pb.HomeChapter>;
  setHomeChaptersList(value: Array<larp_mw5e_other_pb.HomeChapter>): GameState;
  clearHomeChaptersList(): GameState;
  addHomeChapters(value?: larp_mw5e_other_pb.HomeChapter, index?: number): larp_mw5e_other_pb.HomeChapter;

  getSpellsList(): Array<larp_mw5e_other_pb.Spell>;
  setSpellsList(value: Array<larp_mw5e_other_pb.Spell>): GameState;
  clearSpellsList(): GameState;
  addSpells(value?: larp_mw5e_other_pb.Spell, index?: number): larp_mw5e_other_pb.Spell;

  serializeBinary(): Uint8Array;
  toObject(includeInstance?: boolean): GameState.AsObject;
  static toObject(includeInstance: boolean, msg: GameState): GameState.AsObject;
  static serializeBinaryToWriter(message: GameState, writer: jspb.BinaryWriter): void;
  static deserializeBinary(bytes: Uint8Array): GameState;
  static deserializeBinaryFromReader(message: GameState, reader: jspb.BinaryReader): GameState;
}

export namespace GameState {
  export type AsObject = {
    giftsList: Array<larp_mw5e_gifts_pb.Gift.AsObject>,
    skillsList: Array<larp_mw5e_skills_pb.SkillDefinition.AsObject>,
    occupationsList: Array<larp_mw5e_occupations_pb.Occupation.AsObject>,
    vantagesList: Array<larp_mw5e_other_pb.Vantage.AsObject>,
    religionsList: Array<larp_mw5e_other_pb.Religion.AsObject>,
    homeChaptersList: Array<larp_mw5e_other_pb.HomeChapter.AsObject>,
    spellsList: Array<larp_mw5e_other_pb.Spell.AsObject>,
  }
}

