import { NoteColor } from './note-color';

export interface NoteUpdateInputModel {
  id: number;
  title: string;
  content: string;
  color: NoteColor;
  updatedAt: string | null;
}
