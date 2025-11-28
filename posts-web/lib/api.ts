const COMMAND_API = 'http://localhost:5262/api/v1';
const QUERY_API = 'http://localhost:5263/api/v1';

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
export async function createPost(data: CreatePostRequest): Promise<{ id: string }> {
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

export async function deletePost(postId: string): Promise<void> {
  const response = await fetch(`${COMMAND_API}/posts/${postId}`, {
    method: 'DELETE',
  });
  if (!response.ok) throw new Error('Failed to delete post');
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
  return response.json();
}

export async function getPostById(postId: string): Promise<Post> {
  const response = await fetch(`${QUERY_API}/posts/${postId}`);
  if (!response.ok) throw new Error('Failed to fetch post');
  return response.json();
}

export async function getPostsByAuthor(author: string): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts/by-author/${author}`);
  if (!response.ok) throw new Error('Failed to fetch posts by author');
  return response.json();
}

export async function getPostsWithComments(): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts/with-comments`);
  if (!response.ok) throw new Error('Failed to fetch posts with comments');
  return response.json();
}

export async function getPostsWithLikes(numberOfLikes: number): Promise<Post[]> {
  const response = await fetch(`${QUERY_API}/posts/with-likes/${numberOfLikes}`);
  if (!response.ok) throw new Error('Failed to fetch posts with likes');
  return response.json();
}
