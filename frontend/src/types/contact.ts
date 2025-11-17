export interface CreateContactDTO {
  email?: string | null;
  phoneNumber?: string | null;
  title: string;
  content: string;
}