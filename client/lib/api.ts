const COMMAND_API = process.env.NEXT_PUBLIC_COMMAND_API_URL || 'http://localhost:5262/api/v1';
const QUERY_API = process.env.NEXT_PUBLIC_QUERY_API_URL || 'http://localhost:5263/api/v1';

export interface Post {
  postId: string;
  author: string;
  datePosted: string;
  message: string;
  likes: number;
  comments: Comment[];
}

export interface Comment {
  commentId: string;
  username: string;
  commentDate: string;
  comment: string;
  edited: boolean;
}

export interface CreatePostRequest {
  author: string;
  message: string;
}

export interface EditMessageRequest {
  message: string;
}

export interface AddCommentRequest {
  username: string;
  comment: string;
}

export interface EditCommentRequest {
  username: string;
  comment: string;
}

// Command API (write operations)
export async function createPost(data: CreatePostRequest): Promise<{ id: string; message?: string }> {
  const response = await fetch(`${COMMAND_API}/posts`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error('Failed to create post');
  return response.json();
}

export async function editPost(postId: string, data: EditMessageRequest): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}/message`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error('Failed to edit post');
}

export async function likePost(postId: string): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}/like`, {
    method: 'PUT',
  });
  if (!response.ok) throw new Error('Failed to like post');
}

export async function deletePost(postId: string, username: string): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}`, {
    method: 'DELETE',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username }),
  });
  if (!response.ok) {
    const errorText = await response.text();
    throw new Error(`Failed to delete post: ${errorText || response.statusText}`);
  }
}

export async function addComment(postId: string, data: AddCommentRequest): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}/comments`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error('Failed to add comment');
}

export async function editComment(postId: string, commentId: string, data: EditCommentRequest): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}/comments/${commentId}`, {
    method: 'PUT',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  if (!response.ok) throw new Error('Failed to edit comment');
}

export async function removeComment(postId: string, commentId: string, username: string): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}/comments/${commentId}`, {
    method: 'DELETE',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ username }),
  });
  if (!response.ok) throw new Error('Failed to remove comment');
}

// Query API (read operations)
export async function getAllPosts(): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts`);
  if (!response.ok) throw new Error('Failed to fetch posts');
  const data = await response.json();
  return data.posts || [];
}

export async function getPostById(postId: string): Promise<Post> {
  const response = await fetch(`${QUERY_API}/posts/${postId}`);
  if (!response.ok) throw new Error('Failed to fetch post');
  const data = await response.json();
  return data.posts?.[0] || null;
}

export async function getPostsByAuthor(author: string): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts/by-author/${author}`);
  if (!response.ok) throw new Error('Failed to fetch posts by author');
  const data = await response.json();
  return data.posts || [];
}

export async function getPostsWithComments(): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts/with-comments`);
  if (!response.ok) throw new Error('Failed to fetch posts with comments');
  const data = await response.json();
  return data.posts || [];
}

export async function getPostsWithLikes(numberOfLikes: number): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts/with-likes/${numberOfLikes}`);
  if (!response.ok) throw new Error('Failed to fetch posts with likes');
  const data = await response.json();
  return data.posts || [];
}

// Admin API
export async function restoreReadDb(restoreToDateTime?: string): Promise<void> {
  const response = await fetch(`${COMMAND_API}/admin/restore-read-db`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ restoreToDateTime }),
  });
  if (!response.ok) throw new Error('Failed to restore database');
}

export async function hardReset(resetToDateTime: string): Promise<void> {
  const response = await fetch(`${COMMAND_API}/admin/hard-reset`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ resetToDateTime }),
  });
  if (!response.ok) throw new Error('Failed to perform hard reset');
}
