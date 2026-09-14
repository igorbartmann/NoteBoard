import { NoteColor } from './NoteColor';

export interface NoteUpdateInputModel {
  id: number;
  title: string;
  content: string;
  color: NoteColor;
  updatedAt: string | null;
}
