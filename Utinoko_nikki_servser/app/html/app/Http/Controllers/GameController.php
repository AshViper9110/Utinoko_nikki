<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class GameController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $item = DB::table('games')
                ->select('id', 'name')
                ->where('id', $id)
                ->first();

            return response()->json($item);
        }

        $items = DB::table('games')
            ->select('id', 'name')
            ->get();

        return response()->json($items);
    }

    // 家族ユーザー登録
    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255'
        ]);

        // データ登録
        $id = DB::table('games')->insertGetId([
            'name' => $validated['name'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録したユーザーを取得
        $game = DB::table('games')
            ->select('id', 'name')
            ->where('id', $id)
            ->first();

        return response()->json($game, 201);
    }
}
