import type { SessionState } from '../types'

export function ResultsPanel({
  results,
}: {
  results: NonNullable<SessionState['currentQuestionResults']>
}) {
  return (
    <div className="panel">
      <div className="panel-header">
        <h3>Eredmény</h3>
      </div>
      <div className="result-list">
        {results.answers.map((answer) => (
          <div className="result-row" key={answer.answerOptionId}>
            <div className="result-copy">
              <strong>{answer.text}</strong>
              <span>{answer.isCorrect ? 'helyes válasz' : 'válaszlehetőség'}</span>
            </div>
          </div>
        ))}
      </div>
    </div>
  )
}
