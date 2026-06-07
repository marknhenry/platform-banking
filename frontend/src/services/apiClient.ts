export type AuthSession = {
  sessionId: string
  userId: string
  status: 'active' | 'expired' | 'revoked'
  assuranceLevel: 'low' | 'medium' | 'high'
  correlationId: string
}

export type ConsentItem = {
  scopeCategory: string
  status: 'granted' | 'denied' | 'pending' | 'revoked'
  effectiveAt: string
}

export type ConsentProfile = {
  userId: string
  readOnlyMode: true
  consents: ConsentItem[]
}

export type ConsentUpdateResult = {
  scopeCategory: string
  status: string
  enforcementBy: string
}

export type TrustIndicatorResponse = {
  userId: string
  readableScopes: string[]
  blockedScopes: string[]
  lastConsentUpdateAt: string
  readOnlyMode: boolean
}

const fallbackConsentState: Record<string, ConsentProfile> = {}

const allScopes = ['accounts', 'transactions', 'cards', 'payments', 'support']

const createFallbackProfile = (userId: string): ConsentProfile => ({
  userId,
  readOnlyMode: true,
  consents: allScopes.map((scopeCategory) => ({
    scopeCategory,
    status: 'pending',
    effectiveAt: new Date().toISOString(),
  })),
})

const getFallbackProfile = (userId: string): ConsentProfile => {
  if (!fallbackConsentState[userId]) {
    fallbackConsentState[userId] = createFallbackProfile(userId)
  }

  return fallbackConsentState[userId]
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

export async function getConsentProfile(userId: string): Promise<ConsentProfile> {
  try {
    return await request<ConsentProfile>('/v1/consents', {
      headers: {
        'x-user-id': userId,
      },
    })
  } catch {
    return getFallbackProfile(userId)
  }
}

export async function updateConsent(
  userId: string,
  scopeCategory: string,
  status: 'granted' | 'denied' | 'revoked',
): Promise<ConsentUpdateResult> {
  try {
    return await request<ConsentUpdateResult>(`/v1/consents/${scopeCategory}`, {
      method: 'PUT',
      headers: {
        'x-user-id': userId,
      },
      body: JSON.stringify({ status }),
    })
  } catch {
    const profile = getFallbackProfile(userId)
    profile.consents = profile.consents.map((consent) =>
      consent.scopeCategory === scopeCategory
        ? { ...consent, status, effectiveAt: new Date().toISOString() }
        : consent,
    )

    return {
      scopeCategory,
      status,
      enforcementBy: new Date(Date.now() + 30_000).toISOString(),
    }
  }
}

export async function getTrustIndicator(userId: string): Promise<TrustIndicatorResponse> {
  try {
    return await request<TrustIndicatorResponse>('/v1/consents/trust-indicator', {
      headers: {
        'x-user-id': userId,
      },
    })
  } catch {
    const profile = getFallbackProfile(userId)
    const readableScopes = profile.consents
      .filter((consent) => consent.status === 'granted')
      .map((consent) => consent.scopeCategory)
    const blockedScopes = profile.consents
      .filter((consent) => consent.status !== 'granted')
      .map((consent) => consent.scopeCategory)

    return {
      userId,
      readableScopes,
      blockedScopes,
      lastConsentUpdateAt: new Date().toISOString(),
      readOnlyMode: true,
    }
  }
}
