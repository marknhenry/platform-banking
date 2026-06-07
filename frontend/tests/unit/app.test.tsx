import { fireEvent, render, screen } from '@testing-library/react'
import { afterEach, beforeEach, describe, expect, it } from 'vitest'
import App from '../../src/App'

describe('issue 61 frontend shell', () => {
  beforeEach(() => {
    window.localStorage.clear()
    document.documentElement.removeAttribute('data-theme')
  })

  afterEach(() => {
    document.documentElement.removeAttribute('data-theme')
  })

  it('shows welcome content on home by default', async () => {
    render(<App />)

    expect(
      screen.getByRole('heading', { name: /welcome to platform banking/i }),
    ).toBeInTheDocument()
    expect(
      screen.getByText(/conversational, trustworthy banking experience/i),
    ).toBeInTheDocument()
    expect(await screen.findByRole('heading', { name: /trust indicator/i })).toBeInTheDocument()
  })

  it('navigates between Home, About, and Contact pages', async () => {
    render(<App />)

    await screen.findByRole('heading', { name: /trust indicator/i })

    fireEvent.click(screen.getByRole('button', { name: /about/i }))
    expect(screen.getByRole('heading', { name: /about us/i })).toBeInTheDocument()

    fireEvent.click(screen.getByRole('button', { name: /contact us/i }))
    expect(screen.getByText(/support@platform-banking.example/i)).toBeInTheDocument()

    fireEvent.click(screen.getByRole('button', { name: /^home$/i }))
    expect(
      screen.getByRole('heading', { name: /welcome to platform banking/i }),
    ).toBeInTheDocument()
  })

  it('toggles theme mode and persists it', async () => {
    render(<App />)

    await screen.findByRole('heading', { name: /trust indicator/i })

    const toggle = screen.getByRole('button', { name: /switch to dark mode/i })
    fireEvent.click(toggle)

    expect(document.documentElement.getAttribute('data-theme')).toBe('dark')
    expect(window.localStorage.getItem('theme-mode')).toBe('dark')
    expect(
      screen.getByRole('button', { name: /switch to light mode/i }),
    ).toBeInTheDocument()
  })
})
