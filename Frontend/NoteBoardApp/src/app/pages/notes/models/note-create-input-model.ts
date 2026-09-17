import { NoteColor } from './note-color';

export interface NoteCreateInputModel {
  title: string;
  content: string;
  color: NoteColor;
}
