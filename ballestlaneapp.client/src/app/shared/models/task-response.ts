export interface TaskResponse {
  id: string;
  title: string;
  description: string;
  status: number;
  dueDate: string;
  userId: string;
  userEmail: string;
  createdAt: string;
  updatedAt?: string | null;
}
