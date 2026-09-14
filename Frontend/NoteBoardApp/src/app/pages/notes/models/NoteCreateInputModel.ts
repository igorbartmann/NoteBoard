import { NoteColor } from './NoteColor';

export interface NoteCreateInputModel {
  title: string;
  content: string;
  color: NoteColor;
}
