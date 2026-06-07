export type AuthSession = {
  sessionId: string
  userId: string
  status: 'active' | 'expired' | 'revoked'
  assuranceLevel: 'low' | 'medium' | 'high'
  correlationId: string
}

const defaultHeaders = () => ({
  'Content-Type': 'application/json',
  'x-correlation-id': crypto.randomUUID(),
})

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const response = await fetch(path, {
    ...init,
    headers: {
      ...defaultHeaders(),
      ...(init.headers ?? {}),
    },
  })

  if (!response.ok) {
    throw new Error(`API request failed: ${response.status} ${response.statusText}`)
  }

  return (await response.json()) as T
}

export const apiClient = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, {
      method: 'POST',
      body: JSON.stringify(body),
    }),
}

export async function bootstrapAuthSession(): Promise<AuthSession | null> {
  if (typeof fetch !== 'function') {
    return null
  }

  try {
    return await apiClient.get<AuthSession>('/session')
  } catch {
    return null
  }
}
