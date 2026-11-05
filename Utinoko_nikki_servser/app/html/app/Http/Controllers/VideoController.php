<?php

namespace App\Http\Controllers;

use Illuminate\Http\Request;
use Illuminate\Http\JsonResponse;
use Illuminate\Support\Facades\DB;

class VideoController extends Controller
{
    public function index(Request $request): JsonResponse
    {
        $id = $request->query('id');

        if ($id !== null && $id !== '') {
            $item = DB::table('videos')
                ->select('id', 'name', 'url','created_at', 'updated_at')
                ->where('id', $id)
                ->first();

            return response()->json($item);
        }

        $items = DB::table('videos')
            ->select('id', 'name', 'url','created_at', 'updated_at')
            ->get();

        return response()->json($items);
    }

    public function store(Request $request): JsonResponse
    {
        // バリデーション
        $validated = $request->validate([
            'name' => 'required|string|max:255'
        ]);

        // データ登録
        $id = DB::table('videos')->insertGetId([
            'name' => $validated['name'],
            'url' => $request['url'],
            'created_at' => now(),
            'updated_at' => now(),
        ]);

        // 登録したデータを取得
        $game = DB::table('videos')
            ->select('id', 'name', 'url','created_at', 'updated_at')
            ->where('id', $id)
            ->first();

        return response()->json($game, 201);
    }
}
