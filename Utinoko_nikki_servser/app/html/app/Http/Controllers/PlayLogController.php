<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class PlayLogController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $query = DB::table('play_logs')
            ->select(
                'id',
                'elder_user_id',
                'game_id',
                'score',
                'created_at',
                'updated_at'
            );

        // 履歴IDで絞り込み
        if ($request->filled('id')) {
            $item = $query
                ->where('id', $request->query('id'))
                ->first();

            return response()->json($item);
        }

        // 高齢者IDで絞り込み
        if ($request->filled('elder_user_id')) {
            $query->where('elder_user_id', $request->query('elder_user_id'));
        }

        // ゲームIDで絞り込み
        if ($request->filled('game_id')) {
            $query->where('game_id', $request->query('game_id'));
        }

        $items = $query
            ->orderBy('created_at', 'desc')
            ->get();

        return response()->json($items);
    }

    // 登録
    public function store(Request $request): JsonResponse
    {
        $validated = $request->validate([
            'elder_user_id' => 'required|integer|exists:elder_users,id',
            'game_id' => 'required|integer|exists:games,id',
            'score' => 'required|integer',
        ]);

        $id = DB::table('play_logs')->insertGetId([
            'elder_user_id' => $validated['elder_user_id'],
            'game_id' => $validated['game_id'],
            'score' => $validated['score'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        $item = DB::table('play_logs')
            ->select(
                'id',
                'elder_user_id',
                'game_id',
                'score',
                'created_at',
                'updated_at'
            )
            ->where('id', $id)
            ->first();

        return response()->json($item, 201);
    }
}
