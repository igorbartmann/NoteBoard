import { NoteColor } from './note-color';

export interface NoteViewModel {
  id: number;
  title: string;
  content: string;
  isCompleted: boolean;
  color: NoteColor;
  createdAt: string;
  createdBy: number;
  updatedAt: string | null;
}
