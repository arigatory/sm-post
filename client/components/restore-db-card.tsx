'use client';

import { useState } from 'react';
import DatePicker from 'react-datepicker';
import 'react-datepicker/dist/react-datepicker.css';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { restoreReadDb, hardReset } from '@/lib/api';

interface RestoreDbCardProps {
  onRestore: () => void;
}

export function RestoreDbCard({ onRestore }: RestoreDbCardProps) {
  const [loading, setLoading] = useState(false);
  const [showConfirm, setShowConfirm] = useState(false);
  const [selectedDateTime, setSelectedDateTime] = useState<Date | null>(null);
  const [useDateTime, setUseDateTime] = useState(false);
  const [operationType, setOperationType] = useState<'restore' | 'hard-reset'>('restore');

  const handleRestore = async () => {
    setLoading(true);
    try {
      const dateTimeToRestore = useDateTime && selectedDateTime ? selectedDateTime.toISOString() : undefined;
      await restoreReadDb(dateTimeToRestore);
      setShowConfirm(false);
      setSelectedDateTime(null);
      setUseDateTime(false);
      const message = dateTimeToRestore 
        ? `Database restore initiated! Replaying events up to ${selectedDateTime?.toLocaleString()}.`
        : 'Database restore initiated! This will replay all events from the event store.';
      alert(message);
      setTimeout(() => {
        onRestore();
        setLoading(false);
      }, 1500);
    } catch (error) {
      console.error('Failed to restore database:', error);
      alert('Failed to restore database');
      setLoading(false);
    }
  };

  const handleHardReset = async () => {
    if (!selectedDateTime) return;
    
    const confirmed = confirm(
      `⚠️ ОПАСНАЯ ОПЕРАЦИЯ!\n\nВы уверены, что хотите выполнить Hard Reset?\n\n` +
      `Это НЕОБРАТИМО удалит все события после ${selectedDateTime.toLocaleString()}.\n\n` +
      `После этого вы останетесь в этой временной точке и не сможете вернуть удаленные данные!`
    );
    
    if (!confirmed) return;
    
    setLoading(true);
    try {
      await hardReset(selectedDateTime.toISOString());
      setShowConfirm(false);
      setSelectedDateTime(null);
      setUseDateTime(false);
      alert(`Hard Reset completed! All events after ${selectedDateTime.toLocaleString()} have been permanently deleted.`);
      setTimeout(() => {
        onRestore();
        setLoading(false);
      }, 1500);
    } catch (error) {
      console.error('Failed to perform hard reset:', error);
      alert('Failed to perform hard reset');
      setLoading(false);
    }
  };

  return (
    <Card className="border-orange-200 bg-orange-50">
      <CardHeader>
        <CardTitle className="flex items-center gap-2 text-orange-900">
          <span>🔧</span>
          <span>Admin: Restore Read Database</span>
        </CardTitle>
      </CardHeader>
      <CardContent className="space-y-4">
        <p className="text-sm text-orange-800">
          <strong>Restore</strong>: Пересобрать read database из событий (безопасно, event store не меняется)<br/>
          <strong>Hard Reset</strong>: Удалить события после указанного времени (необратимо!)
        </p>
        
        {!showConfirm ? (
          <div className="flex gap-2">
            <Button 
              onClick={() => { setShowConfirm(true); setOperationType('restore'); }} 
              variant="outline" 
              className="border-orange-400 text-orange-700 hover:bg-orange-100"
            >
              🔄 Restore Database
            </Button>
            <Button 
              onClick={() => { setShowConfirm(true); setOperationType('hard-reset'); }} 
              variant="destructive" 
              className="bg-red-600 hover:bg-red-700"
            >
              ⚠️ Hard Reset
            </Button>
          </div>
        ) : (
          <div className="space-y-4 p-4 border border-orange-300 rounded bg-white">
            <div className="flex items-center justify-between">
              <p className="text-sm font-semibold text-orange-900">
                {operationType === 'restore' ? '🔄 Restore Database' : '⚠️ Hard Reset (ОПАСНО!)'}
              </p>
            </div>
            
            {operationType === 'hard-reset' && (
              <div className="p-3 bg-red-50 border border-red-200 rounded text-sm text-red-800">
                <strong>⚠️ Внимание!</strong> Hard Reset необратимо удалит все события после выбранного времени из event store!
              </div>
            )}
            
            <div className="space-y-3">
              <label className="flex items-center gap-2 text-sm font-medium">
                <input
                  type="checkbox"
                  checked={useDateTime}
                  onChange={(e) => setUseDateTime(e.target.checked)}
                  className="rounded"
                />
                <span>Указать дату и время</span>
              </label>
              
              {useDateTime && (
                <div className="space-y-2">
                  <DatePicker
                    selected={selectedDateTime}
                    onChange={(date) => setSelectedDateTime(date)}
                    showTimeSelect
                    timeFormat="HH:mm"
                    timeIntervals={15}
                    dateFormat="dd.MM.yyyy HH:mm"
                    maxDate={new Date()}
                    placeholderText="Выберите дату и время"
                    className="w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-orange-500"
                    calendarClassName="shadow-lg"
                    wrapperClassName="w-full"
                  />
                  {selectedDateTime && (
                    <p className="text-xs text-gray-600">
                      Выбрано: {selectedDateTime.toLocaleString('ru-RU')}
                    </p>
                  )}
                </div>
              )}
            </div>
            
            <p className="text-xs text-orange-700">
              {!useDateTime && '📋 Будут обработаны все события с начала времени'}
              {useDateTime && !selectedDateTime && '📅 Выберите дату и время для продолжения'}
              {useDateTime && selectedDateTime && operationType === 'restore' && 
                `📋 Будут воспроизведены события до ${selectedDateTime.toLocaleString('ru-RU')}`}
              {useDateTime && selectedDateTime && operationType === 'hard-reset' && 
                `🗑️ События после ${selectedDateTime.toLocaleString('ru-RU')} будут УДАЛЕНЫ НАВСЕГДА`}
            </p>
            
            <div className="flex gap-2">
              {operationType === 'restore' ? (
                <Button 
                  onClick={handleRestore} 
                  disabled={loading || (useDateTime && !selectedDateTime)}
                  variant="default"
                  className="bg-orange-600 hover:bg-orange-700"
                >
                  {loading ? 'Восстановление...' : 'Подтвердить Restore'}
                </Button>
              ) : (
                <Button 
                  onClick={handleHardReset} 
                  disabled={loading || !useDateTime || !selectedDateTime}
                  variant="destructive"
                  className="bg-red-600 hover:bg-red-700"
                >
                  {loading ? 'Выполнение...' : '⚠️ Подтвердить Hard Reset'}
                </Button>
              )}
              <Button 
                variant="outline" 
                onClick={() => { setShowConfirm(false); setSelectedDateTime(null); setUseDateTime(false); }}
                disabled={loading}
              >
                Отмена
              </Button>
            </div>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
