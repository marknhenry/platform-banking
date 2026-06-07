import { render, screen } from '@testing-library/react'
import { describe, expect, it } from 'vitest'
import TrustIndicator from '../../src/components/TrustIndicator'

describe('US1 trust indicator', () => {
  it('renders read-only state and scope lists', () => {
    render(
      <TrustIndicator
        indicator={{
          userId: 'user-123',
          readOnlyMode: true,
          readableScopes: ['accounts', 'transactions'],
          blockedScopes: ['payments'],
          lastConsentUpdateAt: new Date().toISOString(),
        }}
      />,
    )

    expect(screen.getByRole('heading', { name: /trust indicator/i })).toBeInTheDocument()
    expect(screen.getByText(/read-only/i)).toBeInTheDocument()
    expect(screen.getByText(/accounts, transactions/i)).toBeInTheDocument()
    expect(screen.getByText(/payments/i)).toBeInTheDocument()
  })
})
