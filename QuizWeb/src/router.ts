export type Route =
  | { name: 'home' }
  | { name: 'auth' }
  | { name: 'dashboard' }
  | { name: 'quiz-new' }
  | { name: 'quiz-detail'; quizId: number }
  | { name: 'host-session'; sessionId: number }
  | { name: 'join' }
  | { name: 'play'; joinCode: string }

export type Notice = {
  tone: 'success' | 'error'
  text: string
}

export function parseRoute(hash: string): Route {
  const normalized = hash.replace(/^#/, '') || '/'
  const clean = normalized.startsWith('/') ? normalized : `/${normalized}`
  const segments = clean.split('/').filter(Boolean)

  if (segments.length === 0) return { name: 'home' }
  if (segments[0] === 'auth') return { name: 'auth' }
  if (segments[0] === 'dashboard') return { name: 'dashboard' }
  if (segments[0] === 'quiz' && segments[1] === 'new') return { name: 'quiz-new' }
  if (segments[0] === 'quiz' && segments[1]) {
    return { name: 'quiz-detail', quizId: Number.parseInt(segments[1], 10) }
  }
  if (segments[0] === 'session' && segments[1]) {
    return { name: 'host-session', sessionId: Number.parseInt(segments[1], 10) }
  }
  if (segments[0] === 'join') return { name: 'join' }
  if (segments[0] === 'play' && segments[1]) {
    return { name: 'play', joinCode: segments[1].toUpperCase() }
  }

  return { name: 'home' }
}

export function navigate(path: string): void {
  window.location.hash = path
}

export function formatStage(stage: 'lobby' | 'question-open' | 'question-closed' | 'finished'): string {
  switch (stage) {
    case 'lobby':
      return 'Várakozás'
    case 'question-open':
      return 'Kérdés aktív'
    case 'question-closed':
      return 'Eredmények'
    case 'finished':
      return 'Befejezve'
    default:
      return stage
  }
}

export function toApiMessage(error: unknown): string {
  if (error instanceof Error) return error.message
  return 'Valami hiba történt.'
}
